import React from 'react';
import styles from './Text.module.scss';

interface TextProps {
  children: React.ReactNode;
  size?: 'sm' | 'base' | 'lg' | 'xl' | '2xl' | '3xl' | '4xl';
  color?: 'primary' | 'secondary' | 'accent' | 'error' | 'success';
  weight?: 'normal' | 'medium' | 'semibold' | 'bold';
  as?: 'p' | 'span' | 'div' | 'h1' | 'h2' | 'h3' | 'h4' | 'h5' | 'h6';
  className?: string;
}

export const Text: React.FC<TextProps> = ({
  children,
  size = 'base',
  color = 'primary',
  weight = 'normal',
  as: Component = 'p',
  className,
  ...props
}) => {
  const classes = [
    styles.text,
    styles[`size-${size}`],
    styles[`color-${color}`],
    styles[`weight-${weight}`],
    className
  ].filter(Boolean).join(' ');

  return React.createElement(Component, { ...props, className: classes }, children);
};
