"use client";

import React from 'react';
import { useRouter } from 'next/navigation';
import { Text } from '@/shared/ui/Text';
import { Button } from '@/shared/ui/Button';
import { VStack } from '@/shared/ui/VStack';
import { ChangeCoins } from '@/shared/ui/ChangeCoins';
import { useAppSelector } from '@/store/hooks';
import { selectChange } from '../model/selector';
import { selectChangeCoins } from '../../payment/model/selector';
import styles from "./page.module.scss";

const SuccessPage = () => {
  const router = useRouter();
  const change = useAppSelector(selectChange);
  const changeCoins = useAppSelector(selectChangeCoins);

  const handleGoHome = () => {
    router.push('/');
  };

  return (
    <div className={styles.page}>
      <VStack gap="lg" align="center">
        <div className={styles.successIcon}>✅</div>
        
        <Text size="3xl" weight="bold" color="primary">
          Спасибо за покупку!
        </Text>
        
        <ChangeCoins coins={changeCoins} totalChange={change} />
        
        <Button
          variant="primary"
          size="lg"
          onClick={handleGoHome}
          className={styles.homeButton}
        >
          Каталог напитков
        </Button>
      </VStack>
    </div>
  );
};

export default SuccessPage;
