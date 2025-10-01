import type { Metadata } from "next";
import "@/shared/config/variables.scss";
import { ReduxProvider } from "@/store/ReduxProvider";
import { MachineLock } from "@/shared/components/MachineLock";

export const metadata: Metadata = {
  title: "Beverage Vending Machine",
  description: "Система управления торговым автоматом",
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="ru">
      <body>
        <ReduxProvider>
          <MachineLock>
            {children}
          </MachineLock>
        </ReduxProvider>
      </body>
    </html>
  );
}
