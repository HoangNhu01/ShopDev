FROM nginx:latest

# Install iconv for encoding conversion
RUN apt-get update && apt-get install -y iconv

# Copy the configuration file and convert encoding from UTF-8 to ANSI
COPY nginx.conf /tmp/nginx.conf
RUN iconv -f UTF-8 -t ISO-8859-1 /tmp/nginx.conf -o /etc/nginx/nginx.conf

# Clean up
RUN rm /tmp/nginx.conf
