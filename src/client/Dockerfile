FROM node:24-alpine AS build
WORKDIR /app

COPY package.json package-lock.json ./
RUN npm ci

COPY . .

RUN npm run build -- --configuration production

FROM nginx:stable-alpine

RUN apk add --no-cache gettext

COPY --from=build /app/dist/client/browser /usr/share/nginx/html
COPY nginx.custom.conf /etc/nginx/conf.d/default.conf
COPY config.json.template /usr/share/nginx/html/assets/config.json.template
COPY docker-entrypoint.sh /docker-entrypoint.sh
RUN sed -i 's/\r//' /docker-entrypoint.sh && chmod +x /docker-entrypoint.sh

EXPOSE 80
CMD ["/docker-entrypoint.sh"]