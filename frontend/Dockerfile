FROM node:20-alpine as angular
WORKDIR /ng-app
COPY frontend/package*.json ./
RUN npm ci
COPY frontend .
ARG NG_BUILD_CONFIGURATION
RUN npm run build:$NG_BUILD_CONFIGURATION

FROM nginx:alpine
COPY --from=angular /ng-app/www /usr/share/nginx/html
EXPOSE 80
