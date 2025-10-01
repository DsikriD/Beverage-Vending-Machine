export interface CartItem {
  id: string;
  name: string;
  brand: string;
  price: number;
  imageUrl: string | null;
  quantity: number;
  maxQuantity: number; // Максимальное количество в автомате
}

export interface CartState {
  items: CartItem[];
  totalAmount: number;
  loading: boolean;
  error: string | null;
}