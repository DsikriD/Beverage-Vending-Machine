import React from 'react';
import styles from './VStack.module.scss';

interface VStackProps {
  children: React.ReactNode;
  gap?: 'none' | 'xs' | 'sm' | 'md' | 'lg' | 'xl';
  align?: 'start' | 'center' | 'end' | 'stretch';
  justify?: 'start' | 'center' | 'end' | 'between' | 'around' | 'evenly';
  className?: string;
}

export const VStack: React.FC<VStackProps> = ({
  children,
  gap = 'md',
  align = 'stretch',
  justify = 'start',
  className,
  ...props
}) => {
  const classes = [
    styles.vstack,
    styles[`gap-${gap}`],
    styles[`align-${align}`],
    styles[`justify-${justify}`],
    className
  ].filter(Boolean).join(' ');

  return (
    <div className={classes} {...props}>
      {children}
    </div>
  );
};
