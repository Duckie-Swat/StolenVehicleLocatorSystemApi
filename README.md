# StolenVehicleLocatorSystemApi

## Setup MongoDb Local using docker
- Create docker-compose.yml and using any text editor put following code in
```
version: "3.8"
  
services:
  mongo:
    image: mongo
    container_name: mongo
    ports:
      - 27017:27017
    volumes:
      - mongodbdata:/data/db
volumes:
  mongodbdata:
```
- Close text editor and open command line and using command:
docker-compose up -d
