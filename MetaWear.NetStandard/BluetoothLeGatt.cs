using MbientLab.Warble;
using MbientLab.MetaWear.Impl.Platform;
using System;
using System.Threading.Tasks;

namespace MbientLab.MetaWear.NetStandard {
    class BluetoothLeGatt : IBluetoothLeGatt {
        public ulong BluetoothAddress { get; private set; }
        public Action OnDisconnect { get; set; }

        private readonly Gatt WarbleGatt;
        
        private TaskCompletionSource<bool> DcTaskSource;

        public BluetoothLeGatt(string mac, string hci = null) {
            WarbleGatt = new Gatt(mac, hci) {
                OnDisconnect = status => {
                    if (DcTaskSource != null) {
                        var copy = DcTaskSource;
                        DcTaskSource = null;
                        copy.SetResult(true);
                    }
                    OnDisconnect?.Invoke();
                }
            };

            BluetoothAddress = Convert.ToUInt64(mac.Replace(":", ""), 16);
        }

        public Task DisconnectAsync() {
            DcTaskSource = new TaskCompletionSource<bool>();
            var copy = DcTaskSource;
            WarbleGatt.Disconnect();
            return copy.Task;
        }

        public async Task DiscoverServicesAsync() {
            await WarbleGatt.ConnectAsync();
        }

        public async Task EnableNotificationsAsync(Tuple<Guid, Guid> gattChar, Action<byte[]> handler) {
            var value = WarbleGatt.FindCharacteristic(gattChar.Item2.ToString());
            
            if (value == null) {
                throw new InvalidOperationException(string.Format("Characteristic '{0}' does not exist", gattChar.Item2));
            } else {
                await value.EnableNotifications();
                value.OnNotificationReceived = bytes => handler(bytes);
            }
        }

        public Task<byte[]> ReadCharacteristicAsync(Tuple<Guid, Guid> gattChar) {
            var value = WarbleGatt.FindCharacteristic(gattChar.Item2.ToString());

            if (value == null) {
                throw new InvalidOperationException(string.Format("Characteristic '{0}' does not exist", gattChar.Item2));
            } else {
                return value.ReadAsync();
            }
        }

        public Task<bool> ServiceExistsAsync(Guid serviceGuid) {
            return Task.FromResult(WarbleGatt.ServiceExists(serviceGuid.ToString()));
        }

        public async Task WriteCharacteristicAsync(Tuple<Guid, Guid> gattChar, GattCharWriteType writeType, byte[] bytes) {
            var value = WarbleGatt.FindCharacteristic(gattChar.Item2.ToString());

            if (value == null) {
                throw new InvalidOperationException(string.Format("Characteristic '{0}' does not exist", gattChar.Item2));
            } else {
                if (writeType == GattCharWriteType.WRITE_WITHOUT_RESPONSE) {
                    await value.WriteWithoutResponseAsync(bytes);
                } else {
                    await value.WriteAsync(bytes);
                }
            }
        }
    }
}
