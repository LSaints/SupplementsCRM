import axios from 'axios';

const api = axios.create({
  baseURL: import.meta.env.VITE_API_URL || '/api',
  headers: {
    'Content-Type': 'application/json',
  },
});

const getAuthData = () => {
  try {
    const stored = localStorage.getItem('auth-storage');
    if (!stored) return null;
    const parsed = JSON.parse(stored);
    return parsed.state;
  } catch {
    return null;
  }
};

api.interceptors.request.use((config) => {
  const auth = getAuthData();
  if (auth?.token) {
    config.headers.Authorization = `Bearer ${auth.token}`;
  }
  return config;
});

api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      localStorage.removeItem('auth-storage');
      window.location.href = '/login';
    }
    return Promise.reject(error);
  }
);

export default api;