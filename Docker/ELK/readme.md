1. setup jenkins on docker
    - tạo các thư mục
        - /opt/jenkins/data
    - tạo file `/opt/jenkins/run.sh`
        ```sh
        HOST_PASS=
        docker stop jenkins || true && docker rm jenkins || true
        docker run -d -p 8080:8080 -p 50000:50000 --restart=on-failure -v /opt/jenkins/data:/var/jenkins_home --name jenkins \
        --add-host="hostmachine:172.16.0.31" \
        -e HOST_PASS=$HOST_PASS \
        jenkins/jenkins:lts-jdk11
        ```
    - cài sshpass cho jenkins
        ```bash
        docker exec -u root -it $Container_id bash
        apt-get update
        apt-get install sshpass
        ```
2. build on jenkins
    ```sh
        sshpass -p $HOST_PASS ssh root@hostmachine 'ls; cd /opt/jenkins/data/workspace/data-warehouse-dev/deploy/ELK; cp .env.dev .env; docker compose up -d'
    ```