# --- build ---
FROM node:24-alpine AS build
WORKDIR /app

# Install deps from the lockfile (clean, reproducible).
COPY package.json package-lock.json ./
RUN npm ci

# Build the production bundle -> dist/client/browser
COPY . .
RUN npm run build

# --- serve ---
FROM nginx:alpine
COPY nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/dist/client/browser /usr/share/nginx/html
EXPOSE 80
