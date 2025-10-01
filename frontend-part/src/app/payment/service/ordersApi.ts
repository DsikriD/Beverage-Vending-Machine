import { createApiRequest } from '@/shared/utils/apiClient';

const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL || 'https://localhost:7280';

export interface CreateOrderItem {
  productId: number;
  quantity: number;
}

export interface PaymentCoin {
  denomination: number;
  quantity: number;
}

export interface ValidatePaymentRequest {
  totalAmount: number;
  paidAmount: number;
  coins: PaymentCoin[];
}

export interface PaymentValidationResult {
  isValid: boolean;
  errorMessage?: string;
  change: number;
  changeCoins?: ChangeCoin[];
}

export interface ChangeCoin {
  denomination: number;
  quantity: number;
}

export interface CreateOrderRequest {
  customerName: string;
  customerEmail?: string | null;
  customerPhone?: string | null;
  paymentMethod: number; // Числовое значение enum (0=Cash, 1=Card, 2=Mobile)
  orderItems: CreateOrderItem[];
  paymentCoins: PaymentCoin[]; // Монеты, которые вставил пользователь
}

export interface OrderResponse {
  id: number;
  customerName: string;
  customerEmail?: string;
  customerPhone?: string;
  totalAmount: number;
  status: string;
  paymentMethod: string;
  createdAt: string;
  orderItems: OrderItemResponse[];
}

export interface OrderItemResponse {
  id: number;
  productId: number;
  productName: string;
  brandName: string;
  quantity: number;
  unitPrice: number;
  totalPrice: number;
}

export class OrdersApi {
  private static async request<T>(endpoint: string, options?: RequestInit): Promise<T> {
    // Проверяем, что мы на клиенте
    if (typeof window === 'undefined') {
      throw new Error('API calls are only available on the client side');
    }

    const url = `${API_BASE_URL}/api/orders${endpoint}`;
    
    // Подготавливаем заголовки
    const headers = new Headers({
      'Content-Type': 'application/json',
      'Accept': 'application/json',
      ...options?.headers,
    });
    
        const response = await createApiRequest(url, {
          headers,
          ...options,
        });

      if (!response.ok) {
        if (response.status === 423) {
          throw new Error('MACHINE_OCCUPIED');
        }
        const errorText = await response.text();
        throw new Error(`HTTP error! status: ${response.status}, message: ${errorText}`);
      }

      const result = await response.json();
      return result;
  }

  static async validatePayment(paymentData: ValidatePaymentRequest): Promise<PaymentValidationResult> {
    return this.request<PaymentValidationResult>('/validate-payment', {
      method: 'POST',
      body: JSON.stringify(paymentData),
    });
  }

  static async createOrder(orderData: CreateOrderRequest): Promise<OrderResponse> {
    return this.request<OrderResponse>('', {
      method: 'POST',
      body: JSON.stringify(orderData),
    });
  }

  static async getOrders(): Promise<OrderResponse[]> {
    return this.request<OrderResponse[]>('');
  }

  static async getOrder(id: number): Promise<OrderResponse> {
    return this.request<OrderResponse>(`/${id}`);
  }
}
