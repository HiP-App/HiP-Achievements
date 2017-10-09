FROM microsoft/dotnet:2.0.0-sdk-jessie

RUN apt-get update && apt-get install make gcc -y

RUN wget https://www.openssl.org/source/openssl-1.1.0f.tar.gz -q && tar xzf openssl-1.1.0f.tar.gz && cd openssl-1.1.0f && ./config && make && make install

RUN wget https://fastdl.mongodb.org/linux/mongodb-linux-x86_64-debian81-3.4.9.tgz -q && tar xzf mongodb-linux-x86_64-debian81-3.4.9.tgz

RUN mkdir -p /data/db

RUN mkdir -p /dotnetapp

COPY . /dotnetapp
WORKDIR /dotnetapp/HiP-Achievements

EXPOSE 5000

RUN dotnet restore --no-cache

CMD /dotnetapp/HiP-Achievements/run.sh
