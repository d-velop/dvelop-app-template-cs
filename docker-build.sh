BUILDCONTAINER=vacationprocess_build_deploy_cs
BUILDCONTAINER_VERSION=latest

DENV=
for var in $(cat environment); do
        DENV+=" -e $var=${!var}"
done

if [ "$1" = "it" ]; then
    docker run -it --rm $DENV -v `pwd`:/build --entrypoint /bin/bash $BUILDCONTAINER:$BUILDCONTAINER_VERSION
else
    docker run --rm $DENV -v `pwd`:/build $BUILDCONTAINER:$BUILDCONTAINER_VERSION "$@"
fi
