#!/bin/sh
envsubst '${API_BASE_URL}' \
  < /usr/share/nginx/html/assets/config.json.template \
  > /usr/share/nginx/html/assets/config.json
exec nginx -g 'daemon off;'
