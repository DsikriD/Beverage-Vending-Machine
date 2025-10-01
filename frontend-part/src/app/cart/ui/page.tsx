"use client";

import React from 'react';
import { useRouter } from 'next/navigation';
import { Text } from '@/shared/ui/Text';
import { Button } from '@/shared/ui/Button';
import { VStack } from '@/shared/ui/VStack';
import { HStack } from '@/shared/ui/HStack';
import { CartItem } from '@/shared/ui/CartItem';
import { useAppDispatch, useAppSelector } from '@/store/hooks';
import { 
  selectCartItems,
  selectCartTotalAmount,
  selectIsCartEmpty,
  selectCartItemsCount,
} from '../model/selector';
import { 
  incrementQuantity,
  decrementQuantity,
  updateQuantity,
  removeFromCart,
} from '../model/cartSlice';
import styles from './page.module.scss';

const CartPage = () => {
  const dispatch = useAppDispatch();
  const router = useRouter();
  
  // Селекторы
  const items = useAppSelector(selectCartItems);
  const totalAmount = useAppSelector(selectCartTotalAmount);
  const isEmpty = useAppSelector(selectIsCartEmpty);
  const itemsCount = useAppSelector(selectCartItemsCount);

  // Обработчики
  const handleIncrement = (id: string) => {
    dispatch(incrementQuantity(id));
  };

  const handleDecrement = (id: string) => {
    dispatch(decrementQuantity(id));
  };

  const handleQuantityChange = (id: string, quantity: number) => {
    dispatch(updateQuantity({ id, quantity }));
  };

  const handleRemove = (id: string) => {
    dispatch(removeFromCart(id));
  };

  const handleGoBack = () => {
    router.back();
  };

  const handleProceedToPayment = () => {
    if (!isEmpty) {
      router.push('/payment');
    }
  };

  // Если корзина пуста
  if (isEmpty) {
    return (
      <div className={styles.page}>
        <VStack gap="lg" align="center">
          <Text size="3xl" weight="bold" color="primary">
            Оформление заказа
          </Text>
          
          <div className={styles.emptyCart}>
            <div className={styles.emptyIcon}>🛒</div>
            <Text size="xl" color="secondary">
              У вас нет ни одного товара
            </Text>
            <Text size="lg" color="secondary">
              Вернитесь на страницу каталога
            </Text>
            
            <Button
              variant="primary"
              size="lg"
              onClick={handleGoBack}
              className={styles.backButton}
            >
              Вернуться к каталогу
            </Button>
          </div>
        </VStack>
      </div>
    );
  }

  return (
    <div className={styles.page}>
      <VStack gap="lg" align="stretch">
        {/* Header */}
        <Text size="3xl" weight="bold" color="primary">
          Оформление заказа
        </Text>

        {/* Cart Items */}
        <div className={styles.cartItems}>
          {items.map((item) => (
            <CartItem
              key={item.id}
              item={item}
              onIncrement={handleIncrement}
              onDecrement={handleDecrement}
              onQuantityChange={handleQuantityChange}
              onRemove={handleRemove}
            />
          ))}
        </div>

        {/* Summary */}
        <div className={styles.summary}>
          <div className={styles.summaryInfo}>
            <Text size="lg">
              Товаров: {itemsCount}
            </Text>
            <Text size="2xl" weight="bold" color="primary">
              Итоговая сумма: {totalAmount} руб.
            </Text>
          </div>
        </div>

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
            onClick={handleProceedToPayment}
            disabled={isEmpty}
            className={styles.actionButton}
          >
            Оплата
          </Button>
        </HStack>
      </VStack>
    </div>
  );
};

export default CartPage;