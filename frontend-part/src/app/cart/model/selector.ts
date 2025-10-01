import { createSelector } from '@reduxjs/toolkit';
import { RootState } from '@/store';

// Базовые селекторы
export const selectCartItems = (state: RootState) => state.cart.items;
export const selectCartTotalAmount = (state: RootState) => state.cart.totalAmount;
export const selectCartLoading = (state: RootState) => state.cart.loading;
export const selectCartError = (state: RootState) => state.cart.error;

// Селектор для проверки, пуста ли корзина
export const selectIsCartEmpty = createSelector(
  [selectCartItems],
  (items) => items.length === 0
);

// Селектор для общего количества товаров в корзине
export const selectCartItemsCount = createSelector(
  [selectCartItems],
  (items) => items.reduce((total, item) => total + item.quantity, 0)
);

// Селектор для получения товара по ID
export const selectCartItemById = createSelector(
  [selectCartItems, (state: RootState, id: string) => id],
  (items, id) => items.find(item => item.id === id)
);

// Селектор для проверки, можно ли увеличить количество товара
export const selectCanIncrementQuantity = createSelector(
  [selectCartItemById],
  (item) => item ? item.quantity < item.maxQuantity : false
);

// Селектор для проверки, можно ли уменьшить количество товара
export const selectCanDecrementQuantity = createSelector(
  [selectCartItemById],
  (item) => item ? item.quantity > 1 : false
);
