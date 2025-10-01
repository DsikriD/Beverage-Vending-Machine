import { RootState } from '@/store/index';
import { createSelector } from '@reduxjs/toolkit';

const selectPaymentState = (state: RootState) => state.payment;

export const selectChange = createSelector(
  selectPaymentState,
  (payment) => payment.change
);
