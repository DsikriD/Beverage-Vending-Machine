import { Product, ProductsFilters } from '../types/product';
import { createApiRequest } from '@/shared/utils/apiClient';

const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL || 'https://localhost:7280';

export class ProductsApi {
  private static async request<T>(endpoint: string, options?: RequestInit): Promise<T> {
    // Проверяем, что мы на клиенте
    if (typeof window === 'undefined') {
      throw new Error('API calls are only available on the client side');
    }

    const url = `${API_BASE_URL}/api/products${endpoint}`;
    
    const response = await createApiRequest(url, {
      headers: {
        'Content-Type': 'application/json',
        ...options?.headers,
      },
      ...options,
    });

    if (!response.ok) {
      if (response.status === 423) {
        throw new Error('MACHINE_OCCUPIED');
      }
      throw new Error(`HTTP error! status: ${response.status}`);
    }

    return response.json();
  }

  static async getProducts(filters: ProductsFilters = {}): Promise<Product[]> {
    const params = new URLSearchParams();
    
    if (filters.brand) params.append('brand', filters.brand);
    if (filters.maxPrice) params.append('maxPrice', filters.maxPrice.toString());
    if (filters.available !== undefined) params.append('available', filters.available.toString());
    if (filters.page) params.append('page', filters.page.toString());
    if (filters.pageSize) params.append('pageSize', filters.pageSize.toString());

    const queryString = params.toString();
    const endpoint = queryString ? `?${queryString}` : '';
    
    return this.request<Product[]>(endpoint);
  }

  static async getProduct(id: number): Promise<Product> {
    return this.request<Product>(`/${id}`);
  }

  static async getBrands(): Promise<string[]> {
    return this.request<string[]>('/brands');
  }
}
