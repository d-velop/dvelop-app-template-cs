#!/bin/bash

APPNAME=acme-apptemplatecs
BUILDCONTAINER=registry.invalid/${APPNAME}_build

DENV=
for var in $(cat environment); do
        DENV+=" -e $var=${!var}"
done

echo "Building new docker image ..."
docker build -t ${BUILDCONTAINER} ./buildcontainer > ./buildcontainer/build.log
if [[ $? -eq 0 ]]; then
    echo "done"
else
    echo "error building image"
    exit 1
fi

if [[ "$1" = "it" ]]; then
    docker run -it --rm ${DENV} --mount type=bind,src="$(pwd)",dst=/build --mount type=volume,src=${APPNAME}_nuget,dst=/root/.nuget/packages --entrypoint /bin/bash ${BUILDCONTAINER}
else
    docker run --rm ${DENV} --mount type=bind,src="$(pwd)",dst=/build --mount type=volume,src=${APPNAME}_nuget,dst=/root/.nuget/packages ${BUILDCONTAINER} "$@"
fi