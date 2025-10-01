export interface Coin {
  denomination: number;
  quantity: number;
  available: number; // Доступное количество в автомате
}

export interface PaymentState {
  coins: Coin[];
  totalAmount: number; // Итоговая сумма товаров
  paidAmount: number; // Внесенная сумма
  change: number; // Сдача
  changeCoins: { denomination: number; quantity: number }[]; // Монеты сдачи
  isPaid: boolean;
  canPay: boolean;
  loading: boolean;
  error: string | null;
}

export interface ChangeResult {
  success: boolean;
  changeAmount: number;
  changeCoins: { denomination: number; quantity: number }[];
  message: string;
}
