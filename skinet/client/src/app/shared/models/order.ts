import { Address } from "./user";

export interface OrderToCreate {
  basketId: string;
  deliveryMethodId: string;
  shipToAddress: Address;
}

export interface OrderItem {
  productId: number;
  productName: string;
  pictureUrl: string;
  price: number;
  quantity: number;
}

export interface Order {
  id: number;
  buyerEmail: string;
  orderDate: Date;
  shipToAddres: Address;
  deliveryMethod: string;
  shippingPrice: number;
  orderItems: OrderItem[];
  subTotal: number;
  total: number;
  status: string;
}
