const axios = require("axios");

async function exposurekeyset (endpoint, exposureKeySetId) {

  const instance = axios.create();
  // add start time header in request
  instance.interceptors.request.use((config) => {
    config.headers["request-startTime"] = process.hrtime();
    return config;
  });

  // add duration header into response
  instance.interceptors.response.use((response) => {
    const start = response.config.headers["request-startTime"];
    const end = process.hrtime(start);
    const milliseconds = Math.round(end[0] * 1000 + end[1] / 1000000);
    response.headers["request-duration"] = milliseconds;
    return response;
  });

  const response = await instance({
    method: "get",
    url:
        endpoint +
        "/" +
        exposureKeySetId,
    headers: { Accept: "application/json" },
    responseType: "stream",
    responseEncoding: null,
  });

  return response;
};

module.exports = exposurekeyset;
