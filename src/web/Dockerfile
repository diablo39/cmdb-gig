FROM nginx:1.17

ARG courceCommit="devversion"

ENV SOURCE_COMMIT=${courceCommit}

COPY nginx.conf /etc/nginx/nginx.conf

COPY . /usr/share/nginx/html

RUN echo "window.cmdbVersion = '$SOURCE_COMMIT';" >  /usr/share/nginx/html/app/application-version.js

RUN mkdir /data

