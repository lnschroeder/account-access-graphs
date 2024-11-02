FROM ubuntu:noble-20241011

WORKDIR /source

COPY . /source

RUN apt-get update && \
    apt-get install -y aspnetcore-runtime-8.0 dotnet-sdk-8.0

ENTRYPOINT ["/bin/bash"]
