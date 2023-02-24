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
import { Basket } from 'src/app/shared/models/basket';
import { Address } from 'src/app/shared/models/user';
import { CheckoutService } from '../checkout.service';
import { loadStripe } from '@stripe/stripe-js';

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
  publishableKey =
    'pk_test_51Mc0PxIfd8QYIl1PO0RCR2b2v8n4KFEdFUmjzZh1h1DWAr3xCLQCOpCA8d8eMylRvibFxnfCKKcNmUqP1k9FPrEI00GbbVDlVa';
  public cardErrors: any;

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
          if (event.error) {
            this.cardErrors = event.error.message;
          } else {
            this.cardErrors = null;
          }
        });

        this.cardExpiry = elements.create('cardExpiry');
        this.cardExpiry.mount(this.cardExpiryElement?.nativeElement);
        this.cardExpiry.on('change', (event) => {
          if (event.error) {
            this.cardErrors = event.error.message;
          } else {
            this.cardErrors = null;
          }
        });

        this.cardCvc = elements.create('cardCvc');
        this.cardCvc.mount(this.cardCvcElement?.nativeElement);
        this.cardCvc.on('change', (event) => {
          if (event.error) {
            this.cardErrors = event.error.message;
          } else {
            this.cardErrors = null;
          }
        });
      }
    });
  }

  submitOrder() {
    const basket = this.basketService.getCurrentBasketValue();
    if (!basket) return;

    const orderToCreate = this.getOrderToCreate(basket);
    if (!orderToCreate) return;

    this.checkoutService.createOrder(orderToCreate).subscribe({
      next: (order) => {
        this.toastr.show('Order created successfully');
        console.log(order);
        this.basketService.deleteLocalBasket();
        const navigationExtras: NavigationExtras = { state: order };
        this.router.navigate(['checkout/success'], navigationExtras);
      },
    });
  }

  private getOrderToCreate(basket: Basket) {
    const deliveryMethodId = this.checkoutForm
      .get('deliveryForm')
      ?.get('deliveryMethod')?.value;
    const shipToAddress = this.checkoutForm.get('addressForm')
      ?.value as Address;

    if (!deliveryMethodId || !shipToAddress) return;

    return {
      basketId: basket.id,
      deliveryMethodId: deliveryMethodId,
      shipToAddress: shipToAddress,
    };
  }
}
