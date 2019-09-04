# systemd

## Installation

```bash
adduser --system --no-create-home --group jetblack
sudo cp jetblack-messagebus.service /lib/systemd/system
sudo systemctl daemon-reload
sudo systemctl enable jetblack-messagebus
sudo systemctl start jetblack-messagebus
```