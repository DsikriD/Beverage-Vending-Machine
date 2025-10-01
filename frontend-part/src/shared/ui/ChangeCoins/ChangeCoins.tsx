import React from 'react';
import { Text } from '../Text';
import { VStack } from '../VStack';
import { HStack } from '../HStack';
import styles from './ChangeCoins.module.scss';

interface ChangeCoin {
  denomination: number;
  quantity: number;
}

interface ChangeCoinsProps {
  coins: ChangeCoin[];
  totalChange: number;
}

export const ChangeCoins: React.FC<ChangeCoinsProps> = ({ coins, totalChange }) => {
  // Все возможные номиналы монет
  const allDenominations = [10, 5, 2, 1];
  
  // Создаем массив для отображения всех номиналов, включая те, которых нет в сдаче
  const displayCoins = allDenominations.map(denomination => {
    const coin = coins.find(c => c.denomination === denomination);
    return {
      denomination,
      quantity: coin ? coin.quantity : 0
    };
  });

  return (
    <div className={styles.changeCoins}>
      <VStack gap="md" align="center">
        <Text size="lg" weight="medium" color="primary">
          Пожалуйста, возьмите вашу сдачу:
        </Text>
        
        <Text size="xl" weight="bold" color="success">
          {totalChange} руб.
        </Text>

        <div className={styles.coinsSection}>
          <Text size="lg" weight="medium" color="primary">
            Ваши монеты:
          </Text>
          
          <div className={styles.coinsList}>
            {displayCoins.map(({ denomination, quantity }) => (
              <div key={denomination} className={styles.coinItem}>
                <div className={styles.coinIcon}>
                  <span className={styles.coinNumber}>{denomination}</span>
                </div>
                <Text size="sm" color="secondary">
                  {quantity} шт.
                </Text>
              </div>
            ))}
          </div>
        </div>
      </VStack>
    </div>
  );
};
