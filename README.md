
# Bilbo

A simple bot that has some information and moderation commands.


## Environment Variables

To run this project, you will need to add the following environment variables.

`BILBO_TOKEN`


## Deployment

To deploy this project, you will need the docker.


Pull the image from the docker hub:
```bash
  docker pull combofive/bilbo:latest
```

Run the docker container:
```bash
    docker run -d --name bilbo -e BILBO_TOKEN=YOUR_DISCORD_TOKEN_HERE combofive/bilbo:latest
```
