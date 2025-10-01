import { createSelector } from '@reduxjs/toolkit';
import { RootState } from '@/store';

// Базовые селекторы
export const selectPaymentCoins = (state: RootState) => state.payment.coins;
export const selectTotalAmount = (state: RootState) => state.payment.totalAmount;
export const selectPaidAmount = (state: RootState) => state.payment.paidAmount;
export const selectChange = (state: RootState) => state.payment.change;
export const selectIsPaid = (state: RootState) => state.payment.isPaid;
export const selectCanPay = (state: RootState) => state.payment.canPay;
export const selectPaymentLoading = (state: RootState) => state.payment.loading;
export const selectPaymentError = (state: RootState) => state.payment.error;

// Селектор для проверки, достаточно ли денег
export const selectIsEnoughMoney = createSelector(
  [selectPaidAmount, selectTotalAmount],
  (paidAmount, totalAmount) => paidAmount >= totalAmount
);

// Селектор для цвета суммы внесенных денег
export const selectPaidAmountColor = createSelector(
  [selectIsEnoughMoney],
  (isEnoughMoney) => isEnoughMoney ? 'success' : 'error'
);

// Селектор для получения монеты по номиналу
export const selectCoinByDenomination = createSelector(
  [selectPaymentCoins, (state: RootState, denomination: number) => denomination],
  (coins, denomination) => coins.find(coin => coin.denomination === denomination)
);

// Селектор для проверки, можно ли увеличить количество монеты
export const selectCanIncrementCoin = createSelector(
  [selectCoinByDenomination],
  (coin) => coin ? coin.quantity < coin.available : false
);

// Селектор для проверки, можно ли уменьшить количество монеты
export const selectCanDecrementCoin = createSelector(
  [selectCoinByDenomination],
  (coin) => coin ? coin.quantity > 0 : false
);

// Селектор для получения монет сдачи
export const selectChangeCoins = (state: RootState) => state.payment.changeCoins;