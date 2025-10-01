import React from 'react';
import Image from 'next/image';
import { Button } from '@/shared/ui/Button';
import { Text } from '@/shared/ui/Text';
import { HStack } from '@/shared/ui/HStack';
import { VStack } from '@/shared/ui/VStack';
import { CartItem as CartItemType } from '@/app/cart/types/cart';
import styles from './CartItem.module.scss';

interface CartItemProps {
  item: CartItemType;
  onIncrement: (id: string) => void;
  onDecrement: (id: string) => void;
  onRemove: (id: string) => void;
  onQuantityChange: (id: string, quantity: number) => void;
}

export const CartItem: React.FC<CartItemProps> = ({
  item,
  onIncrement,
  onDecrement,
  onRemove,
  onQuantityChange,
}) => {
  const handleQuantityChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const newQuantity = parseInt(e.target.value) || 1;
    if (newQuantity >= 1 && newQuantity <= item.maxQuantity) {
      onQuantityChange(item.id, newQuantity);
    }
  };

  const handleIncrement = () => {
    if (item.quantity < item.maxQuantity) {
      onIncrement(item.id);
    }
  };

  const handleDecrement = () => {
    if (item.quantity > 1) {
      onDecrement(item.id);
    }
  };

  const handleRemove = () => {
    onRemove(item.id);
  };

  const totalPrice = item.price * item.quantity;

  return (
    <div className={styles.cartItem}>
      <div className={styles.productInfo}>
        <div className={styles.imageContainer}>
          {item.imageUrl ? (
            <Image 
              src={item.imageUrl} 
              alt={item.name}
              width={60}
              height={60}
              className={styles.image}
              onError={(e) => {
                const target = e.target as HTMLImageElement;
                target.style.display = 'none';
                // Создаем placeholder элемент
                const placeholder = target.nextElementSibling as HTMLElement;
                if (placeholder && placeholder.classList.contains(styles.placeholder)) {
                  placeholder.style.display = 'flex';
                }
              }}
            />
          ) : null}
          <div className={styles.placeholder} style={{ display: item.imageUrl ? 'none' : 'flex' }}>
            <span>📦</span>
          </div>
        </div>
        <VStack gap="xs" align="start" className={styles.details}>
          <Text size="lg" weight="medium">
            {item.name}
          </Text>
          <Text size="sm" color="secondary">
            {item.brand}
          </Text>
        </VStack>
      </div>

      <div className={styles.quantitySection}>
        <HStack gap="sm" align="center">
          <Button
            variant="outline"
            size="sm"
            onClick={handleDecrement}
            disabled={item.quantity <= 1}
            className={styles.quantityButton}
          >
            −
          </Button>
          <input
            type="number"
            min="1"
            max={item.maxQuantity}
            value={item.quantity}
            onChange={handleQuantityChange}
            className={styles.quantityInput}
          />
          <Button
            variant="outline"
            size="sm"
            onClick={handleIncrement}
            disabled={item.quantity >= item.maxQuantity}
            className={styles.quantityButton}
          >
            +
          </Button>
        </HStack>
        <Text size="sm" color="secondary" className={styles.maxQuantity}>
          Макс: {item.maxQuantity}
        </Text>
      </div>

      <div className={styles.priceSection}>
        <Text size="lg" weight="medium">
          {totalPrice} руб.
        </Text>
        <Text size="sm" color="secondary">
          {item.price} руб. за шт.
        </Text>
      </div>

      <div className={styles.actionsSection}>
        <Button
          variant="outline"
          size="sm"
          onClick={handleRemove}
          className={styles.removeButton}
        >
          🗑️
        </Button>
      </div>
    </div>
  );
};
