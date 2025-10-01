import { createSlice, createAsyncThunk, PayloadAction } from '@reduxjs/toolkit';
import { PaymentState, Coin } from '../types/payment';
import { OrdersApi, CreateOrderRequest, ValidatePaymentRequest } from '../service/ordersApi';
import { RootState } from '@/store';

// Асинхронный thunk для проверки платежа
export const validatePayment = createAsyncThunk(
  'payment/validatePayment',
  async (paymentData: ValidatePaymentRequest, { rejectWithValue }) => {
    try {
      return await OrdersApi.validatePayment(paymentData);
    } catch (error) {
      return rejectWithValue(error instanceof Error ? error.message : 'Ошибка при проверке платежа');
    }
  }
);

// Асинхронный thunk для создания заказа
export const createOrder = createAsyncThunk(
  'payment/createOrder',
  async (orderData: CreateOrderRequest, { rejectWithValue }) => {
    try {
      return await OrdersApi.createOrder(orderData);
    } catch (error) {
      return rejectWithValue(error instanceof Error ? error.message : 'Ошибка при создании заказа');
    }
  }
);

const initialState: PaymentState = {
  coins: [
    { denomination: 1, quantity: 0, available: 100 },
    { denomination: 2, quantity: 0, available: 100 },
    { denomination: 5, quantity: 0, available: 100 },
    { denomination: 10, quantity: 0, available: 100 },
  ],
  totalAmount: 0,
  paidAmount: 0,
  change: 0,
  changeCoins: [],
  isPaid: false,
  canPay: false,
  loading: false,
  error: null,
};

const paymentSlice = createSlice({
  name: 'payment',
  initialState,
  reducers: {
    setTotalAmount: (state, action: PayloadAction<number>) => {
      state.totalAmount = action.payload;
      state.change = state.paidAmount - state.totalAmount;
      state.canPay = state.paidAmount >= state.totalAmount && state.totalAmount > 0;
    },
    
    updateCoinQuantity: (state, action: PayloadAction<{
      denomination: number;
      quantity: number;
    }>) => {
      const { denomination, quantity } = action.payload;
      const coin = state.coins.find(c => c.denomination === denomination);
      
      if (coin && quantity >= 0 && quantity <= coin.available) {
        coin.quantity = quantity;
        
        // Пересчитываем внесенную сумму
        state.paidAmount = state.coins.reduce((total, c) => total + (c.denomination * c.quantity), 0);
        state.change = state.paidAmount - state.totalAmount;
        state.canPay = state.paidAmount >= state.totalAmount && state.totalAmount > 0;
      }
    },
    
    incrementCoinQuantity: (state, action: PayloadAction<number>) => {
      const denomination = action.payload;
      const coin = state.coins.find(c => c.denomination === denomination);
      
      if (coin && coin.quantity < coin.available) {
        coin.quantity += 1;
        state.paidAmount = state.coins.reduce((total, c) => total + (c.denomination * c.quantity), 0);
        state.change = state.paidAmount - state.totalAmount;
        state.canPay = state.paidAmount >= state.totalAmount && state.totalAmount > 0;
      }
    },
    
    decrementCoinQuantity: (state, action: PayloadAction<number>) => {
      const denomination = action.payload;
      const coin = state.coins.find(c => c.denomination === denomination);
      
      if (coin && coin.quantity > 0) {
        coin.quantity -= 1;
        state.paidAmount = state.coins.reduce((total, c) => total + (c.denomination * c.quantity), 0);
        state.change = state.paidAmount - state.totalAmount;
        state.canPay = state.paidAmount >= state.totalAmount && state.totalAmount > 0;
      }
    },
    
    setCoins: (state, action: PayloadAction<Coin[]>) => {
      state.coins = action.payload;
      state.paidAmount = state.coins.reduce((total, c) => total + (c.denomination * c.quantity), 0);
      state.change = state.paidAmount - state.totalAmount;
      state.canPay = state.paidAmount >= state.totalAmount && state.totalAmount > 0;
    },
    
    resetPayment: (state) => {
      state.coins = state.coins.map(coin => ({ ...coin, quantity: 0 }));
      state.paidAmount = 0;
      state.change = 0;
      state.changeCoins = [];
      state.isPaid = false;
      state.canPay = false;
      state.error = null;
    },
    
    setPaid: (state) => {
      state.isPaid = true;
    },
    
    setLoading: (state, action: PayloadAction<boolean>) => {
      state.loading = action.payload;
    },
    
    setError: (state, action: PayloadAction<string | null>) => {
      state.error = action.payload;
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(validatePayment.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
          .addCase(validatePayment.fulfilled, (state, action) => {
            state.loading = false;
            if (action.payload.isValid) {
              state.error = null;
              state.change = action.payload.change;
              state.changeCoins = action.payload.changeCoins || [];
            } else {
              state.error = action.payload.errorMessage || 'Ошибка при проверке платежа';
              state.changeCoins = [];
            }
          })
      .addCase(validatePayment.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload as string;
      })
      .addCase(createOrder.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
          .addCase(createOrder.fulfilled, (state) => {
            state.loading = false;
            state.error = null;
            state.isPaid = true;
            // Монеты сдачи уже сохранены из validatePayment
          })
      .addCase(createOrder.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload as string;
      });
  },
});

export const {
  setTotalAmount,
  updateCoinQuantity,
  incrementCoinQuantity,
  decrementCoinQuantity,
  setCoins,
  resetPayment,
  setPaid,
  setLoading,
  setError,
} = paymentSlice.actions;

export default paymentSlice.reducer;
