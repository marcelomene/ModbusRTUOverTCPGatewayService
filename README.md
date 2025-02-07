# ModbusRTUOverTCPGatewayService

A Windows desktop service that connects to a ModbusRTU device via a serial port and provides an interface for multiple processes to read and write data from the Modbus device.

The goal is to make a ModbusRTU device, which operates exclusively in a Master-Slave model, accessible over Ethernet to multiple consumers. Essentially, the service converts communication to ModbusRTU over TCP, allowing the device to be accessed by multiple masters simultaneously.

The service manages connection concurrency among multiple clients accessing the serial device, ensuring efficient and secure communication.

Example use case: A ModbusRTU temperature sensor installed in an industrial plant is physically connected to a single computer. With the service running, the sensor's data becomes accessible from multiple points on the network, enabling various systems to retrieve temperature readings simultaneously.
