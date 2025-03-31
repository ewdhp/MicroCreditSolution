#!/bin/bash

# Threshold for inotify usage (percentage of max_user_instances)
THRESHOLD=90
INOTIFY_FILE="/proc/sys/fs/inotify/max_user_instances"
CURRENT_LIMIT=$(cat $INOTIFY_FILE)

# Function to get the current inotify usage
get_inotify_usage() {
    find /proc/*/fd -type l -lname 'anon_inode:inotify' 2>/dev/null | wc -l
}

# Function to clean up .NET-related processes consuming excessive inotify instances
cleanup_dotnet_inotify() {
    echo "Cleaning up .NET-related inotify instances..."
    for pid in $(ps -eo pid,comm | grep dotnet | awk '{print $1}'); do
        echo "Terminating .NET process with PID: $pid"
        kill -9 $pid 2>/dev/null
    done
}

# Monitor inotify usage and clean up if necessary
while true; do
    CURRENT_USAGE=$(get_inotify_usage)
    USAGE_PERCENT=$((CURRENT_USAGE * 100 / CURRENT_LIMIT))

    echo "Current inotify usage: $CURRENT_USAGE / $CURRENT_LIMIT ($USAGE_PERCENT%)"

    if [ $USAGE_PERCENT -ge $THRESHOLD ]; then
        echo "Inotify usage exceeded threshold ($THRESHOLD%). Cleaning up .NET-related processes..."
        cleanup_dotnet_inotify
    fi

    sleep 10 # Check every 10 seconds
done