import React from 'react';
import styles from './HStack.module.scss';

interface HStackProps {
  children: React.ReactNode;
  gap?: 'none' | 'xs' | 'sm' | 'md' | 'lg' | 'xl';
  align?: 'start' | 'center' | 'end' | 'stretch';
  justify?: 'start' | 'center' | 'end' | 'between' | 'around' | 'evenly';
  wrap?: boolean;
  className?: string;
}

export const HStack: React.FC<HStackProps> = ({
  children,
  gap = 'md',
  align = 'center',
  justify = 'start',
  wrap = false,
  className,
  ...props
}) => {
  const classes = [
    styles.hstack,
    styles[`gap-${gap}`],
    styles[`align-${align}`],
    styles[`justify-${justify}`],
    wrap && styles.wrap,
    className
  ].filter(Boolean).join(' ');

  return (
    <div className={classes} {...props}>
      {children}
    </div>
  );
};
