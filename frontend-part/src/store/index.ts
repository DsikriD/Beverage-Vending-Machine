import { configureStore } from '@reduxjs/toolkit';
import catalogReducer from '../app/catalog/model/productsSlice';
import cartReducer from '../app/cart/model/cartSlice';
import paymentReducer from '../app/payment/model/paymentSlice';

export const store = configureStore({
  reducer: {
    catalog: catalogReducer,
    cart: cartReducer,
    payment: paymentReducer,
  },
});

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;
