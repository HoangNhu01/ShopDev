#!/bin/bash
set -e

# Chờ SQL Server khởi động
/opt/mssql/bin/sqlservr &

# Lưu PID của quá trình SQL Server
pid=$!

# Chờ SQL Server sẵn sàng
echo "Waiting for SQL Server to start..."
sleep 30

# Kiểm tra nếu sqlcmd tồn tại
if ! [ -x "$(command -v /opt/mssql-tools/bin/sqlcmd)" ]; then
    echo "Error: sqlcmd is not installed or not in PATH." >&2
    exit 1
fi

# Thực thi script cấu hình
echo "Running configure_alwayson.sql..."
/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "$SA_PASSWORD" -i /usr/configure_alwayson.sql

# Đợi process chính
wait $pid
