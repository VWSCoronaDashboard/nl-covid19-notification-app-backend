const chai = require("chai");
const expect = chai.expect;
const manifest = require("../behaviours/manifest_behaviour");
const exposure_key_set = require("../behaviours/exposure_keys_set_behaviour");
const decode_protobuf = require("../../util/protobuff_decoding");

describe("Manifest endpoints tests #exposurekeyset #endpoints #regression", function () {
  this.timeout(2000 * 60 * 30);

  let manifest_response,exposureKeySetId,exposure_keyset_response,exposure_keyset_decoded;

  describe("I get the manifest", function () {});

    before(function (){
      return manifest().then(function (manifest){
        manifest_response = manifest;
        exposureKeySetId = manifest.content.exposureKeySets[0];
      }).then(async function (){
        return exposure_key_set(exposureKeySetId).then(function (exposure_keyset){
          exposure_keyset_response = exposure_keyset;
          return decode_protobuf(exposure_keyset_response)
            .then(function (EKS){
            exposure_keyset_decoded = EKS;
          });
        })
      })
    });

    it("I have received the Manifest", function () {
      expect(manifest_response.response.status, "response status code").to.be.eql(200);
    });

    it("I have received the EKS", function (){
      expect(exposure_keyset_response.status,"response status code").to.be.eql(200);
      expect(exposure_keyset_response.headers, "response header").to.have.nested.property(
          "content-type",
          "application/zip"
      );
    })

  it("Max-Age of exposureKey data validated", function (){
    let lastModified = Date.parse(exposure_keyset_response.headers["last-modified"]);
    let now = Date.now();
    let maxAge = exposure_keyset_response.headers["cache-control"].split("=");
    expect(
        (now - lastModified) / 1000,
        `Response last-modified (${exposure_keyset_response.headers["last-modified"]} is not older then ${maxAge[1]/3600} hours ago`
    ).to.be.below(parseInt(maxAge[1]));
  });

  it("Exposure Key sets response time is under 5 sec.", function () {
    expect(exposure_keyset_response.headers["request-duration"],"response time is below 5 sec.").to.be.below(5000);
  });

  it("Exposure Key sets has all needed property keys", function () {
    expect(exposure_keyset_decoded).to.have.nested.property("endTimestamp");
    expect(exposure_keyset_decoded).to.have.nested.property("region");
    expect(exposure_keyset_decoded).to.have.nested.property("batchNum");
    expect(exposure_keyset_decoded).to.have.nested.property("batchSize");
    expect(exposure_keyset_decoded).to.have.nested.property("signatureInfos");
    expect(exposure_keyset_decoded).to.have.nested.property("keys");
    expect(exposure_keyset_decoded.signatureInfos[0]).to.have.nested.property("verificationKeyVersion","v10");
    expect(exposure_keyset_decoded.signatureInfos[0]).to.have.nested.property("verificationKeyId","204");
    expect(exposure_keyset_decoded.signatureInfos[0]).to.have.nested.property("signatureAlgorithm","1.2.840.10045.4.3.2");
    expect(exposure_keyset_decoded.keys[0]).to.have.nested.property("keyData").that.is.not.null;
    expect(exposure_keyset_decoded.keys[0]).to.have.nested.property("transmissionRiskLevel").that.is.not.null;
    expect(exposure_keyset_decoded.keys[0]).to.have.nested.property("rollingStartIntervalNumber").that.is.not.null;
    expect(exposure_keyset_decoded.keys[0]).to.have.nested.property("rollingPeriod").that.is.not.null;
  });

});