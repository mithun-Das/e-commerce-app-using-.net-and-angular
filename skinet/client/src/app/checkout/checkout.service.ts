import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs';
import { environment } from 'src/environments/environment';
import { DeliveryMethod } from '../shared/models/deliveryMethod';

@Injectable({
  providedIn: 'root',
})
export class CheckoutService {

  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  getDeliveryMethods() {
    return this.http.get<DeliveryMethod[]>(this.baseUrl + 'Orders/deliveryMethods').pipe(
      map((methods) => {
        return methods.sort((a,b) => a.price - b.price);
      })
    )
  }
}
