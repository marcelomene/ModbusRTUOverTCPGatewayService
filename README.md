Um serviço desktop para Windows que se conecta a um dispositivo ModbusRTU via porta serial e disponibiliza uma interface para que diversos processos possam ler e escrever dados no dispositivo Modbus.

O objetivo é tornar um dispositivo ModbusRTU, que opera exclusivamente no modelo Master-Slave, acessível via Ethernet para múltiplos consumidores. Essencialmente, o serviço converte a comunicação para ModbusRTU over TCP, permitindo que o dispositivo seja acessado por vários mestres simultaneamente.

O serviço gerencia a concorrência entre múltiplos clientes conectados ao dispositivo serial, garantindo acesso eficiente e seguro.

Exemplo de uso: Um sensor de temperatura ModbusRTU instalado em uma planta industrial está fisicamente conectado a um único computador. Com o serviço em execução, os dados do sensor tornam-se acessíveis a diversos outros pontos da rede, permitindo que múltiplos sistemas obtenham as leituras de temperatura de forma simultânea.

--

A Windows desktop service that connects to a ModbusRTU device via a serial port and provides an interface for multiple processes to read and write data from the Modbus device.

The goal is to make a ModbusRTU device, which operates exclusively in a Master-Slave model, accessible over Ethernet to multiple consumers. Essentially, the service converts communication to ModbusRTU over TCP, allowing the device to be accessed by multiple masters simultaneously.

The service manages connection concurrency among multiple clients accessing the serial device, ensuring efficient and secure communication.

Example use case: A ModbusRTU temperature sensor installed in an industrial plant is physically connected to a single computer. With the service running, the sensor's data becomes accessible from multiple points on the network, enabling various systems to retrieve temperature readings simultaneously.
