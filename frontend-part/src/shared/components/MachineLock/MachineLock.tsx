"use client";

import React, { useEffect, useState } from 'react';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { Text } from '@/shared/ui/Text';
import { VStack } from '@/shared/ui/VStack';
import styles from './MachineLock.module.scss';

interface MachineLockProps {
  children: React.ReactNode;
}

export const MachineLock: React.FC<MachineLockProps> = ({ children }) => {
  const [connection, setConnection] = useState<HubConnection | null>(null);
  const [isMachineAvailable, setIsMachineAvailable] = useState<boolean | null>(null);
  const [isConnecting, setIsConnecting] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    let currentConnection: HubConnection | null = null;
    
    const startConnection = async () => {
      try {
        currentConnection = new HubConnectionBuilder()
          .withUrl(`${process.env.NEXT_PUBLIC_API_URL || 'https://localhost:7280'}/vendingMachineHub`)
          .withAutomaticReconnect()
          .build();

        // Обработчики событий
        currentConnection.on('MachineAvailable', () => {
          setIsMachineAvailable(true);
          setError(null);
        });

        currentConnection.on('MachineOccupied', () => {
          setIsMachineAvailable(false);
          setError(null);
        });

        currentConnection.on('MachineBusy', (message: string) => {
          setIsMachineAvailable(false);
          setError(message);
        });

        currentConnection.on('MachineForceReleased', (message: string) => {
          setIsMachineAvailable(false);
          setError(message);
        });

        // Обработка ошибок подключения
        currentConnection.onclose((error) => {
          if (error) {
            setError('Ошибка подключения к серверу');
            setIsMachineAvailable(false);
          }
        });

        await currentConnection.start();
        setConnection(currentConnection);
        setIsConnecting(false);

        // Сохраняем ConnectionId в localStorage для использования в API запросах
        localStorage.setItem('machineConnectionId', currentConnection.connectionId || '');

        // Проверяем статус автомата при подключении
        await currentConnection.invoke('CheckMachineStatus');
      } catch (err) {
        console.error('Ошибка подключения к SignalR:', err);
        setError('Не удалось подключиться к серверу');
        setIsConnecting(false);
        setIsMachineAvailable(false);
      }
    };

    startConnection();

    return () => {
      if (currentConnection) {
        currentConnection.stop();
      }
    };
  }, []); // Убираем connection из зависимостей

  // Состояние подключения
  if (isConnecting) {
    return (
      <div className={styles.overlay}>
        <VStack gap="md" align="center">
          <Text size="xl">Подключение к автомату...</Text>
        </VStack>
      </div>
    );
  }

  // Автомат занят - показываем сообщение
  if (isMachineAvailable === false) {
    return (
      <div className={styles.overlay}>
        <VStack gap="lg" align="center">
          <div className={styles.busyIcon}>🚫</div>
          <Text size="2xl" color="error" weight="bold">
            {error || 'Извините, в данный момент автомат занят'}
          </Text>
          <Text size="lg" color="secondary">
            Пожалуйста, подождите, пока другой пользователь закончит покупку
          </Text>
        </VStack>
      </div>
    );
  }

  // Автомат доступен - показываем содержимое
  if (isMachineAvailable === true) {
    return <>{children}</>;
  }

  // Если статус еще не определен (null), показываем загрузку
  return (
    <div className={styles.overlay}>
      <VStack gap="md" align="center">
        <Text size="xl">Проверка статуса автомата...</Text>
      </VStack>
    </div>
  );
};
