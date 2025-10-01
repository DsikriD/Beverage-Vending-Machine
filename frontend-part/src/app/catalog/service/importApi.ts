import { createApiRequest } from '@/shared/utils/apiClient';

const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL || 'https://localhost:7280';

export class ImportApi {
  private static async request<T>(endpoint: string, options?: RequestInit): Promise<T> {
    // Проверяем, что мы на клиенте
    if (typeof window === 'undefined') {
      throw new Error('API calls are only available on the client side');
    }

    const url = `${API_BASE_URL}/api/import${endpoint}`;

    const response = await createApiRequest(url, {
      headers: {
        ...options?.headers,
      },
      ...options,
    });

    if (!response.ok) {
      if (response.status === 423) {
        throw new Error('MACHINE_OCCUPIED');
      }
      const errorText = await response.text();
      throw new Error(`HTTP error! status: ${response.status}, message: ${errorText}`);
    }

    return response.json();
  }

  static async importProducts(file: File): Promise<{ message: string; products: unknown[] }> {
    const formData = new FormData();
    formData.append('file', file);

    return this.request('/products', {
      method: 'POST',
      body: formData,
    });
  }

  static async downloadTemplate(): Promise<Blob> {
    // Проверяем, что мы на клиенте
    if (typeof window === 'undefined') {
      throw new Error('API calls are only available on the client side');
    }

    const url = `${API_BASE_URL}/api/import/template`;

    const response = await createApiRequest(url, {
      method: 'GET',
    });

    if (!response.ok) {
      if (response.status === 423) {
        throw new Error('MACHINE_OCCUPIED');
      }
      throw new Error(`HTTP error! status: ${response.status}`);
    }

    return response.blob();
  }
}
