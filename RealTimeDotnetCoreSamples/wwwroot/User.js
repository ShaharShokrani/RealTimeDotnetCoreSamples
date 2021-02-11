listen = () => {
    let INDEX = 0;
    var eventSource = new EventSource("/api/Users/UsersSSE");
    //eventSource.onmessage = (event) => {
    //    INDEX++;
    //    if (INDEX == 2) {
    //        this.close()
    //    }
    //    console.log(event.data);
    //}

    eventSource.addEventListener("message", function (e) {
        console.log(e.data)
    })

     //eventSource.close();
}

let intervalId;

poll = (orderId) => {
    fetch(`/Coffee/${orderId}`)
        .then(response => {
            if (response.status === 200) {
                const albumsTable = document.getElementById("albums");
                response.json().then(j => {
                    // Create an empty <tr> element and add it to the 1st position of the table:
                    var row = albumsTable.insertRow(0);

                    // Insert new cells (<td> elements) at the 1st and 2nd position of the "new" <tr> element:
                    var cell1 = row.insertCell(0);
                    cell1.innerHTML = j.UserId;

                    var cell2 = row.insertCell(1);
                    cell2.innerHTML = j.Id;

                    var cell3 = row.insertCell(2);
                    cell3.innerHTML = j.Title;
                                        
                    //if (j.finished)
                    //    clearInterval(intervalId);
                });
            }
        }
        );
}

document.getElementById("submit").addEventListener("click", e => {


    e.preventDefault();

    listen();
    //const product = document.getElementById("product").value;
    //const size = document.getElementById("size").value;
    //fetch("/api/Users/UsersSSE",
    //    {
    //        method: "POST",
    //        body: JSON.stringify({}),
    //        headers: {
    //            'Content-Type': 'application/json'
    //        }
    //    })
    //    .then(response => response.text())
    //    //.then(text => intervalId = setInterval(poll, 1000, text));
    //    .then(j => {
    //        console.log(j);
    //        //const albumsTable = document.getElementById("albums");
    //        //var row = albumsTable.insertRow(0);
    //        //// Insert new cells (<td> elements) at the 1st and 2nd position of the "new" <tr> element:
    //        //var cell1 = row.insertCell(0);
    //        //cell1.innerHTML = j.UserId;

    //        //var cell2 = row.insertCell(1);
    //        //cell2.innerHTML = j.Id;

    //        //var cell3 = row.insertCell(2);
    //        //cell3.innerHTML = j.Title;
    //    })
});