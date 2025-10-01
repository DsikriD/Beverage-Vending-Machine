import React, { useState } from 'react';
import Image from 'next/image';
import { Text } from '../Text';
import { Button } from '../Button';
import styles from './ProductCard.module.scss';

interface ProductCardProps {
  id: string;
  name: string;
  price: number;
  image: string;
  status: 'available' | 'selected' | 'outOfStock';
  onSelect?: (id: string) => void;
  className?: string;
}

export const ProductCard: React.FC<ProductCardProps> = ({
  id,
  name,
  price,
  image,
  status,
  onSelect,
  className
}) => {
  const [imageError, setImageError] = useState(false);
  
  // Проверяем, является ли URL изображения валидным
  const isValidImageUrl = (url: string): boolean => {
    if (!url) return false;
    // Проверяем, что это не локальный путь к несуществующему файлу
    if (url.startsWith('/images/') && !url.includes('http')) {
      return false;
    }
    return true;
  };
  
  const classes = [
    styles.card,
    className
  ].filter(Boolean).join(' ');

  const getButtonProps = () => {
    switch (status) {
      case 'available':
        return {
          variant: 'primary' as const,
          children: 'Выбрать',
          onClick: () => onSelect?.(id)
        };
      case 'selected':
        return {
          variant: 'secondary' as const,
          children: 'Выбрано',
          disabled: true
        };
      case 'outOfStock':
        return {
          variant: 'outline' as const,
          children: 'Закончился',
          disabled: true
        };
      default:
        return {
          variant: 'primary' as const,
          children: 'Выбрать',
          onClick: () => onSelect?.(id)
        };
    }
  };

  return (
    <div className={classes}>
      <div className={styles.imageContainer}>
        {imageError || !image || !isValidImageUrl(image) ? (
          <div className={styles.placeholder}>
            <span>📦</span>
          </div>
          ) : (
            <Image 
              src={image} 
              alt={name} 
              width={200}
              height={150}
              className={styles.image}
              onError={() => setImageError(true)}
            />
          )}
      </div>
      <div className={styles.content}>
        <Text size="sm" color="primary" className={styles.name}>
          {name}
        </Text>
        <Text size="lg" color="primary" weight="semibold" className={styles.price}>
          {price} руб.
        </Text>
        <Button
          size="sm"
          fullWidth
          {...getButtonProps()}
        />
      </div>
    </div>
  );
};
