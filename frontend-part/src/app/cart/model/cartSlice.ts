import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { CartState } from '../types/cart';

const initialState: CartState = {
  items: [],
  totalAmount: 0,
  loading: false,
  error: null,
};

const cartSlice = createSlice({
  name: 'cart',
  initialState,
  reducers: {
    addToCart: (state, action: PayloadAction<{
      id: string;
      name: string;
      brand: string;
      price: number;
      imageUrl: string | null;
      maxQuantity: number;
    }>) => {
      const { id, name, brand, price, imageUrl, maxQuantity } = action.payload;
      const existingItem = state.items.find(item => item.id === id);
      
      if (existingItem) {
        // Если товар уже есть в корзине, увеличиваем количество
        if (existingItem.quantity < maxQuantity) {
          existingItem.quantity += 1;
        }
      } else {
        // Если товара нет в корзине, добавляем его
        state.items.push({
          id,
          name,
          brand,
          price,
          imageUrl,
          quantity: 1,
          maxQuantity,
        });
      }
      
      // Пересчитываем общую сумму
      state.totalAmount = state.items.reduce((total, item) => total + (item.price * item.quantity), 0);
    },
    
    removeFromCart: (state, action: PayloadAction<string>) => {
      state.items = state.items.filter(item => item.id !== action.payload);
      state.totalAmount = state.items.reduce((total, item) => total + (item.price * item.quantity), 0);
    },
    
    updateQuantity: (state, action: PayloadAction<{
      id: string;
      quantity: number;
    }>) => {
      const { id, quantity } = action.payload;
      const item = state.items.find(item => item.id === id);
      
      if (item && quantity > 0 && quantity <= item.maxQuantity) {
        item.quantity = quantity;
        state.totalAmount = state.items.reduce((total, item) => total + (item.price * item.quantity), 0);
      }
    },
    
    incrementQuantity: (state, action: PayloadAction<string>) => {
      const item = state.items.find(item => item.id === action.payload);
      if (item && item.quantity < item.maxQuantity) {
        item.quantity += 1;
        state.totalAmount = state.items.reduce((total, item) => total + (item.price * item.quantity), 0);
      }
    },
    
    decrementQuantity: (state, action: PayloadAction<string>) => {
      const item = state.items.find(item => item.id === action.payload);
      if (item && item.quantity > 1) {
        item.quantity -= 1;
        state.totalAmount = state.items.reduce((total, item) => total + (item.price * item.quantity), 0);
      }
    },
    
    clearCart: (state) => {
      state.items = [];
      state.totalAmount = 0;
    },
    
    setLoading: (state, action: PayloadAction<boolean>) => {
      state.loading = action.payload;
    },
    
    setError: (state, action: PayloadAction<string | null>) => {
      state.error = action.payload;
    },
  },
});

export const {
  addToCart,
  removeFromCart,
  updateQuantity,
  incrementQuantity,
  decrementQuantity,
  clearCart,
  setLoading,
  setError,
} = cartSlice.actions;

export default cartSlice.reducer;
