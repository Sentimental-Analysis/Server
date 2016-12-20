FROM microsoft/dotnet:1.1.0-sdk-projectjson

COPY . /var/www/sentiment

WORKDIR /var/www/sentiment

RUN ["dotnet", "restore"]

RUN ["dotnet", "build"]
# RUN ["dotnet", "ef", "database", "update"]

EXPOSE 5000/tcp

CMD ["dotnet", "run", "--server.urls", "http://*:5000"]