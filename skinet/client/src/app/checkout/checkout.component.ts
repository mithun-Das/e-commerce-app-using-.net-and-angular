import { Component, OnInit } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { AccountService } from '../account/account.service';
import { BasketService } from '../basket/basket.service';

@Component({
  selector: 'app-checkout',
  templateUrl: './checkout.component.html',
  styleUrls: ['./checkout.component.scss'],
})
export class CheckoutComponent implements OnInit {
  constructor(
    private fb: FormBuilder,
    private accountService: AccountService,
    private basketService: BasketService
  ) {}

  checkOutForm = this.fb.group({
    addressForm: this.fb.group({
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      street: ['', Validators.required],
      city: ['', Validators.required],
      state: ['', Validators.required],
      zipcode: ['', Validators.required],
    }),
    deliveryForm: this.fb.group({
      deliveryMethod: ['', Validators.required],
    }),
    paymentForm: this.fb.group({
      nameOnCard: ['', Validators.required],
    }),
  });

  ngOnInit(): void {
    this.getAddressFromValues();
    this.getDeliveryMethodValue();
  }

  getAddressFromValues() {
    this.accountService.getUserAddress().subscribe({
      next: (address) => {
        address && this.checkOutForm.get('addressForm')?.patchValue(address);
      },
    });
  }

  getDeliveryMethodValue() {
    const basket = this.basketService.getCurrentBasketValue();
    if (basket && basket.deliveryMethodId) {
      this.checkOutForm
        .get('deliveryForm')
        ?.get('deliveryMethod')
        .patchValue(basket.deliveryMethodId.toString());
    }
  }
}
