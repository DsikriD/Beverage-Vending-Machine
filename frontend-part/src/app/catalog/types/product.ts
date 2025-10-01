export interface Product {
  id: number;
  name: string;
  brand: string;
  price: number;
  imageUrl?: string;
  isAvailable: boolean;
  stockQuantity: number;
  status: 'available' | 'selected' | 'outOfStock';
}

export interface ProductsResponse {
  products: Product[];
  totalCount: number;
  currentPage: number;
  totalPages: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
}

export interface ProductsFilters {
  brand?: string;
  maxPrice?: number;
  available?: boolean;
  page?: number;
  pageSize?: number;
}

export interface Brand {
  id: number;
  name: string;
}

export interface CatalogState {
  products: Product[];
  brands: string[];
  selectedProducts: string[];
  filters: ProductsFilters;
  pagination: {
    currentPage: number;
    totalPages: number;
    totalCount: number;
    hasNextPage: boolean;
    hasPreviousPage: boolean;
  };
  loading: boolean;
  error: string | null;
  importLoading: boolean;
  importError: string | null;
}
