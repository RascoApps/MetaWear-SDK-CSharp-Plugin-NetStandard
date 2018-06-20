﻿using MbientLab.MetaWear.Impl;
using MbientLab.MetaWear.Impl.Platform;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MbientLab.MetaWear.NetStandard {
    /// <summary>
    /// Entry point into the MetaWear API for .NET Core applications
    /// </summary>
    public class Application {
        private static Dictionary<string, Tuple<MetaWearBoard, BluetoothLeGatt, IO>> btleDevices = new Dictionary<string, Tuple<MetaWearBoard, BluetoothLeGatt, IO>>();
        /// <summary>
        /// Root path that the API uses to cache data
        /// </summary>
        public static string CacheRoot { get; set; }
        static Application() {
            CacheRoot = ".metawear"; 
        }

        private class IO : ILibraryIO {
            private readonly string MacAddr;

            public IO(string macAddr) {
                MacAddr = macAddr.Replace(":", "");
            }

            public async Task<Stream> LocalLoadAsync(string key) {
                return await Task.FromResult(File.Open(Path.Combine(Directory.GetCurrentDirectory(), CacheRoot, MacAddr, key), FileMode.Open));
            }

            public Task LocalSaveAsync(string key, byte[] data) {
                Console.WriteLine(Directory.GetCurrentDirectory());
                var root = Path.Combine(Directory.GetCurrentDirectory(), CacheRoot, MacAddr);
                if (!Directory.Exists(root)) {
                    Directory.CreateDirectory(root);
                }
                using (Stream outs = File.Open(Path.Combine(root, key), FileMode.Create)) {
                    outs.Write(data, 0, data.Length);
                }
                return Task.CompletedTask;
            }
        }

        /// <summary>
        /// Instantiates an <see cref="IMetaWearBoard"/> object corresponding to mac address
        /// </summary>
        /// <param name="mac"></param>
        /// <returns></returns>
        public static IMetaWearBoard GetMetaWearBoard(string mac) {
            if (btleDevices.TryGetValue(mac, out Tuple<MetaWearBoard, BluetoothLeGatt, IO> value)) {
                return value.Item1;
            }

            var gatt = new BluetoothLeGatt(mac);
            var io = new IO(mac);
            value = Tuple.Create(new MetaWearBoard(gatt, io), gatt, io);
            btleDevices.Add(mac, value);
            return value.Item1;
        }
        /// <summary>
        /// Removes the <see cref="IMetaWearBoard"/> object corresponding to the mac address
        /// </summary>
        /// <param name="mac">Mac address of the target object</param>
        public static void RemoveMetaWearBoard(string mac) {
            if (btleDevices.TryGetValue(mac, out Tuple<MetaWearBoard, BluetoothLeGatt, IO> value)) {
                value.Item2.DisconnectAsync();
                btleDevices.Remove(mac);
            }
        }
        /// <summary>
        /// Clears cached information corresponding to the mac address
        /// </summary>
        /// <param name="mac">Mac address of the information to remove</param>
        /// <returns></returns>
        public static async Task ClearDeviceCacheAsync(string mac) {
            var path = Path.Combine(CacheRoot, mac);

            File.Delete(path);
            await Task.CompletedTask;
        }
    }
}
