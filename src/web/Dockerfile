FROM nginx:1.17

ENV SOURCE_COMMIT="devversion"

COPY nginx.conf /etc/nginx/nginx.conf

COPY . /usr/share/nginx/html

RUN echo "window.cmdbVersion = '${SOURCE_COMMIT:-devversion}';" >  /usr/share/nginx/html/app/application-version.js

RUN mkdir /data

