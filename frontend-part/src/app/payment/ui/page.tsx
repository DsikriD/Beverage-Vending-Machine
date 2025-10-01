"use client";

import React, { useEffect } from 'react';
import { useRouter } from 'next/navigation';
import { Text } from '@/shared/ui/Text';
import { Button } from '@/shared/ui/Button';
import { VStack } from '@/shared/ui/VStack';
import { HStack } from '@/shared/ui/HStack';
import { CoinRow } from '@/shared/ui/CoinRow';
import { useAppDispatch, useAppSelector } from '@/store/hooks';
import { 
  selectPaymentCoins,
  selectTotalAmount,
  selectPaidAmount,
  selectChange,
  selectCanPay,
  selectPaymentLoading,
  selectPaymentError,
  selectPaidAmountColor,
} from '../model/selector';
import { 
  setTotalAmount,
  incrementCoinQuantity,
  decrementCoinQuantity,
  updateCoinQuantity,
  setPaid,
  setLoading,
  setError,
  createOrder,
  validatePayment,
  resetPayment,
} from '../model/paymentSlice';
import { selectCartTotalAmount } from '../../cart/model/selector';
import { clearCart } from '../../cart/model/cartSlice';
import { RootState } from '@/store';
import styles from './page.module.scss';

const PaymentPage = () => {
  const dispatch = useAppDispatch();
  const router = useRouter();
  
  // Селекторы
  const coins = useAppSelector(selectPaymentCoins);
  const totalAmount = useAppSelector(selectTotalAmount);
  const paidAmount = useAppSelector(selectPaidAmount);
  const change = useAppSelector(selectChange);
  const canPay = useAppSelector(selectCanPay);
  const loading = useAppSelector(selectPaymentLoading);
  const error = useAppSelector(selectPaymentError);
  const paidAmountColor = useAppSelector(selectPaidAmountColor);
  const cartTotalAmount = useAppSelector(selectCartTotalAmount);
  const cartItems = useAppSelector((state: RootState) => state.cart.items);

  // Сбрасываем монеты и устанавливаем итоговую сумму при заходе на страницу
  useEffect(() => {
    // Сбрасываем все монеты при заходе на страницу
    dispatch(resetPayment());
    
    // Устанавливаем итоговую сумму из корзины
    if (cartTotalAmount > 0) {
      dispatch(setTotalAmount(cartTotalAmount));
    }
  }, [dispatch, cartTotalAmount]);

  // Обработчики
  const handleIncrement = (denomination: number) => {
    dispatch(incrementCoinQuantity(denomination));
  };

  const handleDecrement = (denomination: number) => {
    dispatch(decrementCoinQuantity(denomination));
  };

  const handleQuantityChange = (denomination: number, quantity: number) => {
    dispatch(updateCoinQuantity({ denomination, quantity }));
  };

  const handleGoBack = () => {
    router.back();
  };

  const handlePay = async () => {
    if (!canPay) return;
    
    try {
      
      // Проверяем, что корзина не пуста
      if (cartItems.length === 0) {
        dispatch(setError('Корзина пуста'));
        return;
      }
      
      
      // Сначала проверяем возможность выдачи сдачи
      const paymentValidationData = {
        totalAmount: totalAmount,
        paidAmount: paidAmount,
        coins: coins.map(coin => ({
          denomination: coin.denomination,
          quantity: coin.quantity
        }))
      };
      
      
      const validationResult = await dispatch(validatePayment(paymentValidationData)).unwrap();
      
      
      if (!validationResult.isValid) {
        // Если сдачу выдать нельзя, показываем конкретную ошибку
        dispatch(setError(validationResult.errorMessage || 'Невозможно выдать сдачу'));
        return;
      }
      
      // Если проверка прошла успешно, создаем заказ
      const orderData = {
        customerName: 'Покупатель', // Можно сделать форму для ввода данных
        customerEmail: null, // Заменяем undefined на null
        customerPhone: null, // Заменяем undefined на null
        paymentMethod: 0, // Cash = 0 в enum
        orderItems: cartItems.map(item => ({
          productId: parseInt(item.id),
          quantity: item.quantity
        })),
        paymentCoins: coins.map(coin => ({
          denomination: coin.denomination,
          quantity: coin.quantity
        }))
      };
      
      
      // Отправляем заказ на сервер
      const orderResult = await dispatch(createOrder(orderData)).unwrap();
      
      // Очищаем корзину только после успешного создания заказа
      dispatch(clearCart());
      
      // Переходим на страницу успеха
      router.push('/success');
    } catch (error) {
      dispatch(setError(error instanceof Error ? error.message : 'Ошибка при обработке платежа'));
    }
  };

  return (
    <div className={styles.page}>
      <VStack gap="lg" align="stretch">
        {/* Header */}
        <Text size="3xl" weight="bold" color="primary">
          Оплата
        </Text>

        {/* Coins Table */}
        <div className={styles.coinsTable}>
          <div className={styles.tableHeader}>
            <Text size="lg" weight="medium">Номинал</Text>
            <Text size="lg" weight="medium">Количество</Text>
            <Text size="lg" weight="medium">Сумма</Text>
          </div>
          
          <div className={styles.tableBody}>
            {coins.map((coin) => (
              <CoinRow
                key={coin.denomination}
                coin={coin}
                onIncrement={handleIncrement}
                onDecrement={handleDecrement}
                onQuantityChange={handleQuantityChange}
              />
            ))}
          </div>
        </div>

        {/* Summary */}
        <div className={styles.summary}>
          <div className={styles.summaryRow}>
            <Text size="lg" weight="medium">
              Итоговая сумма:
            </Text>
            <Text size="xl" weight="bold" color="primary">
              {totalAmount} руб.
            </Text>
          </div>
          
          <div className={styles.summaryRow}>
            <Text size="lg" weight="medium">
              Вы внесли:
            </Text>
            <Text size="xl" weight="bold" color={paidAmountColor}>
              {paidAmount} руб.
            </Text>
          </div>
          
          {change > 0 && (
            <div className={styles.summaryRow}>
              <Text size="lg" weight="medium">
                Сдача:
              </Text>
              <Text size="lg" weight="medium" color="success">
                {change} руб.
              </Text>
            </div>
          )}
        </div>

        {/* Error */}
        {error && (
          <div className={styles.error}>
            <Text size="sm" color="error">{error}</Text>
          </div>
        )}

        {/* Actions */}
        <HStack justify="between" align="center">
          <Button
            variant="secondary"
            size="lg"
            onClick={handleGoBack}
            className={styles.actionButton}
          >
            Вернуться
          </Button>
          
          <Button
            variant="primary"
            size="lg"
            onClick={handlePay}
            disabled={!canPay || loading}
            className={styles.actionButton}
          >
            {loading ? 'Обработка...' : 'Оплатить'}
          </Button>
        </HStack>
      </VStack>
    </div>
  );
};

export default PaymentPage;