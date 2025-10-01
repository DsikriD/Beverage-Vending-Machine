import React from 'react';
import { Button } from '@/shared/ui/Button';
import { Text } from '@/shared/ui/Text';
import { HStack } from '@/shared/ui/HStack';
import { Coin } from '@/app/payment/types/payment';
import styles from './CoinRow.module.scss';

interface CoinRowProps {
  coin: Coin;
  onIncrement: (denomination: number) => void;
  onDecrement: (denomination: number) => void;
  onQuantityChange: (denomination: number, quantity: number) => void;
}

export const CoinRow: React.FC<CoinRowProps> = ({
  coin,
  onIncrement,
  onDecrement,
  onQuantityChange,
}) => {
  const handleQuantityChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const newQuantity = parseInt(e.target.value) || 0;
    if (newQuantity >= 0 && newQuantity <= coin.available) {
      onQuantityChange(coin.denomination, newQuantity);
    }
  };

  const handleIncrement = () => {
    if (coin.quantity < coin.available) {
      onIncrement(coin.denomination);
    }
  };

  const handleDecrement = () => {
    if (coin.quantity > 0) {
      onDecrement(coin.denomination);
    }
  };

  const amount = coin.denomination * coin.quantity;

  return (
    <div className={styles.coinRow}>
      <div className={styles.denomination}>
        <div className={styles.coinIcon}>
          <span>{coin.denomination}</span>
        </div>
        <Text size="sm" weight="medium">
          {coin.denomination} {coin.denomination === 1 ? 'рубль' : 
           coin.denomination < 5 ? 'рубля' : 'рублей'}
        </Text>
      </div>

      <div className={styles.quantity}>
        <HStack gap="sm" align="center">
          <Button
            variant="outline"
            size="sm"
            onClick={handleDecrement}
            disabled={coin.quantity <= 0}
            className={styles.quantityButton}
          >
            −
          </Button>
          <input
            type="number"
            min="0"
            max={coin.available}
            value={coin.quantity}
            onChange={handleQuantityChange}
            className={styles.quantityInput}
          />
          <Button
            variant="outline"
            size="sm"
            onClick={handleIncrement}
            disabled={coin.quantity >= coin.available}
            className={styles.quantityButton}
          >
            +
          </Button>
        </HStack>
      </div>

      <div className={styles.amount}>
        <Text size="sm" weight="medium">
          {amount} руб.
        </Text>
      </div>
    </div>
  );
};
