-- Tạo endpoint trên cả primary và secondary
CREATE ENDPOINT [Hadr_endpoint] 
AS TCP (LISTENER_PORT = 5022)
FOR DATA_MIRRORING (
    ROLE = ALL,
    AUTHENTICATION = WINDOWS NEGOTIATE
)

-- Tạo Availability Group
CREATE AVAILABILITY GROUP [YourAG]
WITH (AUTOMATED_BACKUP_PREFERENCE = PRIMARY)
FOR 
DATABASE ShopDevDB
REPLICA ON 
    'sqlserver_primary' WITH (
        ENDPOINT_URL = 'TCP://sqlserver_primary:5022',
        AVAILABILITY_MODE = SYNCHRONOUS_COMMIT,
        FAILOVER_MODE = AUTOMATIC
    ),
    'sqlserver_secondary' WITH (
        ENDPOINT_URL = 'TCP://sqlserver_secondary:5022',
        AVAILABILITY_MODE = SYNCHRONOUS_COMMIT,
        FAILOVER_MODE = AUTOMATIC
    )

EXEC sp_configure 'hadr_enabled', 1;
RECONFIGURE;