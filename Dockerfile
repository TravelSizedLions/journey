FROM gableroux/unity3d:2020.1.1f1-windows

WORKDIR /

RUN wget http://mirrors.kernel.org/ubuntu/pool/main/a/aptitude/aptitude-common_0.8.10-6ubuntu1_all.deb
RUN dpkg -i aptitude-common_0.8.10-6ubuntu1_all.deb

RUN apt update

RUN apt-get install software-properties-common -y && apt-get update

RUN add-apt-repository ppa:git-core/ppa -y && \
    apt update && \
    apt-get install git -y

CMD ["/bin/sh"]
