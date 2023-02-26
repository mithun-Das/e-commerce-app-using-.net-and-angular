import { Component, OnInit, Input, ViewChild, ElementRef } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { NavigationExtras, Router } from '@angular/router';
import { Stripe } from '@stripe/stripe-js';
import {
  StripeCardCvcElement,
  StripeCardExpiryElement,
  StripeCardNumberElement,
} from '@stripe/stripe-js/types/stripe-js';
import { ToastrService } from 'ngx-toastr';
import { BasketService } from 'src/app/basket/basket.service';
import { Basket, IBasket } from 'src/app/shared/models/basket';
import { Address } from 'src/app/shared/models/user';
import { CheckoutService } from '../checkout.service';
import { loadStripe } from '@stripe/stripe-js';
import { firstValueFrom } from 'rxjs';
import { OrderToCreate } from 'src/app/shared/models/order';

@Component({
  selector: 'app-checkout-payment',
  templateUrl: './checkout-payment.component.html',
  styleUrls: ['./checkout-payment.component.scss'],
})
export class CheckoutPaymentComponent implements OnInit {
  @Input() checkoutForm: FormGroup;
  @ViewChild('cardNumber') cardNumberElement: ElementRef;
  @ViewChild('cardExpiry') cardExpiryElement: ElementRef;
  @ViewChild('cardCvc') cardCvcElement: ElementRef;
  stripe: Stripe | null = null;
  cardNumber?: StripeCardNumberElement;
  cardExpiry?: StripeCardExpiryElement;
  cardCvc?: StripeCardCvcElement;
  cardNumberComplete = false;
  cardExpiryComplete = false;
  cardCvcComplete = false;
  publishableKey =
    'pk_test_51Mc0PxIfd8QYIl1PO0RCR2b2v8n4KFEdFUmjzZh1h1DWAr3xCLQCOpCA8d8eMylRvibFxnfCKKcNmUqP1k9FPrEI00GbbVDlVa';
  public cardErrors: any;
  loading = false;

  constructor(
    private basketService: BasketService,
    private checkoutService: CheckoutService,
    private toastr: ToastrService,
    private router: Router
  ) {}

  ngOnInit(): void {
    loadStripe(this.publishableKey).then((stripe) => {
      this.stripe = stripe;
      console.log(this.stripe);
      const elements = this.stripe?.elements();
      if (elements) {
        this.cardNumber = elements.create('cardNumber');
        this.cardNumber.mount(this.cardNumberElement?.nativeElement);
        this.cardNumber.on('change', (event) => {
          this.cardNumberComplete = event.complete;
          if (event.error) {
            this.cardErrors = event.error.message;
          } else {
            this.cardErrors = null;
          }
        });

        this.cardExpiry = elements.create('cardExpiry');
        this.cardExpiry.mount(this.cardExpiryElement?.nativeElement);
        this.cardExpiry.on('change', (event) => {
          this.cardExpiryComplete = event.complete;
          if (event.error) {
            this.cardErrors = event.error.message;
          } else {
            this.cardErrors = null;
          }
        });

        this.cardCvc = elements.create('cardCvc');
        this.cardCvc.mount(this.cardCvcElement?.nativeElement);
        this.cardCvc.on('change', (event) => {
          this.cardCvcComplete = event.complete;
          if (event.error) {
            this.cardErrors = event.error.message;
          } else {
            this.cardErrors = null;
          }
        });
      }
    });
  }

  get paymentFormComplete() {
    return (
      this.checkoutForm?.get('paymentForm')?.valid &&
      this.cardNumberComplete &&
      this.cardExpiryComplete &&
      this.cardCvcComplete
    );
  }

  private async createOrder(basket: Basket | null) {
    if (!basket) throw new Error('Basket is null');
    const orderToCreate = this.getOrderToCreate(basket);
    return firstValueFrom(this.checkoutService.createOrder(orderToCreate));
  }

  private async confirmPaymentWithStripe(basket: IBasket | null) {
    if (!basket) throw new Error('Basket is null');

    const result = this.stripe?.confirmCardPayment(basket.clientSecret!, {
      payment_method: {
        card: this.cardNumber!,
        billing_details: {
          name: this.checkoutForm?.get('paymentForm')?.get('nameOnCard')?.value,
        },
      },
    });

    if (!result) throw new Error('Problem attempting payment with stripe');
    return result;
  }

  async submitOrder() {
    this.loading = true;
    const basket = this.basketService.getCurrentBasketValue();
    try {
      const createdOrder = await this.createOrder(basket);
      const paymentResult = await this.confirmPaymentWithStripe(basket);
      if (paymentResult.paymentIntent) {
        this.basketService.deleteLocalBasket();
        const navigationExtras: NavigationExtras = { state: createdOrder };
        this.router.navigate(['checkout/success'], navigationExtras);
      } else {
        this.toastr.error(paymentResult.error.message);
      }
    } catch (error: any) {
      console.log(error);
      this.toastr.show(error.message);
    } finally {
      this.loading = false;
    }
  }

  private getOrderToCreate(basket: Basket): OrderToCreate {
    const deliveryMethodId = this.checkoutForm
      .get('deliveryForm')
      ?.get('deliveryMethod')?.value;
    const shipToAddress = this.checkoutForm.get('addressForm')
      ?.value as Address;

    if (!deliveryMethodId || !shipToAddress)
      throw new Error('Problem with basket');

    return {
      basketId: basket.id,
      deliveryMethodId: deliveryMethodId,
      shipToAddress: shipToAddress,
    };
  }
}
