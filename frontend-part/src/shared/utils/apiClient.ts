// Утилита для создания fetch запросов с ConnectionId заголовком
export const createApiRequest = async (
  url: string,
  options: RequestInit = {}
): Promise<Response> => {
  const connectionId = localStorage.getItem("machineConnectionId");

  // Создаем новый Headers объект, чтобы не мутировать исходный
  const headers = new Headers(options.headers);
  if (connectionId) {
    headers.set("X-Connection-Id", connectionId);
  }

  const response = await fetch(url, {
    ...options,
    headers, // Используем наш Headers объект
  });

  return response;
};
